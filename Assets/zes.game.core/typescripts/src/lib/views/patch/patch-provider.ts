import compareVersions from "compare-versions";
import { Liv } from "csharp";
import { singleton } from "tsyringe";
import { ApplicationRef } from "../../core/application-ref";
import { getLogger } from "../../logger";
import { isNullOrEmpty, waitForSeconds, waitRequest } from "../../util";
import { PatchFileInfo, PatchInfo } from "./patchinfo";
import { VersionInfo } from "./versioninfo";
import _ = require("lodash");

export enum PatchStatus {
    None = 0,
    Extract,
    Found,
    Reinstall,
}

@singleton()
export class PatchProvider {
    constructor(
        private app: ApplicationRef
    ) { }

    async check(): Promise<PatchStatus> {
        const config = this.app.host.config;
        const http = this.app.http;
        if (!config.checkUpdate) {
            await waitForSeconds(1);
            return PatchStatus.None;
        }

        const streamingJson: string = await waitRequest(Liv.Patch.PatchUtil.loadLocalVersionInfo(true));
        const persistentJson: string = await waitRequest(Liv.Patch.PatchUtil.loadLocalVersionInfo(false)).catch(() => "");
        if (isNullOrEmpty(persistentJson)) {
            return PatchStatus.Extract; // 本地没有记录，需要解压
        }
        const streaming = <VersionInfo>JSON.parse(streamingJson);
        const local = <VersionInfo>JSON.parse(persistentJson);
        if (compareVersions(streaming.version, local.version) > 0) {
            return PatchStatus.Extract; // 包里的版本新，需要解压
        }

        // get remote version
        const remoteJson: string = await waitRequest(http.get(local.url));
        const remote = <VersionInfo>JSON.parse(remoteJson);
        const result = compareVersions(local.version, remote.version);
        if (result < 0) {

            if (compareVersions(local.version, remote.minVersion) < 0) {
                return PatchStatus.Reinstall;
            }

            return PatchStatus.Found;
        }
        return PatchStatus.None;
    }

    async extract() {
        const util = Liv.Patch.PatchUtil;
        util.clearPersistentDir(Liv.Patch.PatchConstants.patchDir);
        await waitRequest(util.copyFromStreamingToPatch(Liv.Patch.PatchConstants.versionInfoFile));
        await waitRequest(util.copyFromStreamingToPatch(Liv.Patch.PatchConstants.patchInfoFile));
    }

    async patch(progress: (p: number) => void) {
        const http = this.app.http;
        const util = Liv.Patch.PatchUtil;
        const piJson: string = await waitRequest(util.loadLocalPatchInfo());
        const local = <PatchInfo>JSON.parse(piJson);
        const remotePatchJson: string = await waitRequest(http.get(local.url + "/" + Liv.Patch.PatchConstants.patchInfoFile));
        const remote = <PatchInfo>JSON.parse(remotePatchJson);
        const remoteVersionJson: string = await waitRequest(http.get(local.url + "/" + Liv.Patch.PatchConstants.versionInfoFile));

        const map = new Map<string, PatchFileInfo>();
        const list: PatchFileInfo[] = [];
        for (const item of local.files) {
            map.set(item.path, item);
        }
        for (const item of remote.files) {
            if (!map.has(item.path)) {
                list.push(item);
            } else {
                const localItem = map.get(item.path);
                if (localItem?.md5 != item.md5) {
                    list.push(item);
                }
            }
        }
        list.forEach(i => logger.info(`find patch file ${i.path}`));

        if (list.length <= 0) {
            logger.warn(`no patch file found`);
            return;
        }

        const totalsize = _.sumBy(list, (i) => i.size);
        let downloaded = 0;
        util.clearPersistentDir(Liv.Patch.PatchConstants.patchTempDir);
        for (const item of list) {
            await waitRequest(http.download(`${remote.url}/${item.path}`, Liv.Patch.PatchConstants.patchTempDir, item.path, (p: number) => {
                const downloading = p * item.size;
                progress((downloaded + downloading) / totalsize);
            }))
            downloaded += item.size;
            progress(downloaded / totalsize);
        }
        await waitForSeconds(0.1);

        for (const item of list) {
            logger.info(`copy patch file ${item.path} to ${Liv.Patch.PatchConstants.patchDir}`);
            await waitRequest(util.copyFromTempToPatch(item.path));
        }

        util.clearPersistentDir(Liv.Patch.PatchConstants.patchTempDir);
        progress(1);

        // save version and patch info files
        util.saveFile(Liv.Patch.PatchConstants.patchInfoFile, remotePatchJson);
        util.saveFile(Liv.Patch.PatchConstants.versionInfoFile, remoteVersionJson);
    }
}

const logger = getLogger(PatchProvider.name);
