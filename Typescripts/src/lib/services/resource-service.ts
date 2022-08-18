import { Liv, System, UnityEngine } from "csharp";
import { $typeof } from "puerts";
import { singleton } from "tsyringe";
import { getLogger } from "../logger";
import { waitRequest } from "../util";

const bundles = [
    "data",
    "icons",
    "scenes",
    "effects",
    "ui-prefabs",
    "ui-sprites",
    "map_main",
    "videos",
    "mod-h",
];


@singleton()
export class ResourceService {

    constructor() {
        this.loader = Liv.App.loader;
    }

    private loader: Liv.IResourceLoader;
    private assets: Map<string, UnityEngine.Object> = new Map();

    async loadAsset<T extends UnityEngine.Object>(path: string, type: System.Type): Promise<T> {
        if (this.assets.has(path)) {
            const ret = this.assets.get(path);
            if (!ret) {
                throw new Error(`get Object of null (${path})`);
            }
            return <T>ret;
        }
        const req = this.loader.loadAsset(path, type);
        const ret = await waitRequest(req);
        this.assets.set(path, ret);
        return ret;
    }

    async loadPrefab(path: string): Promise<UnityEngine.Object> {
        return this.loadAsset(path, $typeof(UnityEngine.Object));
    }

    async loadScene(name: string): Promise<void> {
        await waitRequest(this.loader.loadScene(name));
    }

    async loadAdditiveScene(name: string, progress: (p: number) => void) {
        const scene = await waitRequest<UnityEngine.SceneManagement.Scene>(this.loader.loadAdditiveScene(name, progress));
        return scene;
    }

    unloadScene(scene: UnityEngine.SceneManagement.Scene) {
        return waitRequest<void>(this.loader.unloadScene(scene));
    }

    loadBundle(name: string, onProgress?: (p: number) => void): Promise<void> {
        logger.debug(`loading bundle ${name}`);
        return waitRequest(this.loader.loadBundle(name, onProgress || (() => { /** nothing */ })))
            .catch(logger.error.bind(logger));
    }

    loadBundles(progress: (p: number) => void): Promise<void> {
        const progs = bundles.map(() => 0);
        return Promise.all(bundles.map((v, i) => {
            return this.loadBundle(v, (p: number) => {
                progs[i] = p;
                const value = progs.reduce((a, b) => a + b);
                progress(value / bundles.length);
            });
        })).then(() => progress(1));
    }

}

const logger = getLogger(ResourceService.name);
