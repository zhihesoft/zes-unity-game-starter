import { UnityEngine } from "csharp";
import { container } from "tsyringe";
import { getLogger } from "../logger";
import { ResourceService } from "../services/resource-service";
import { isNullOrEmpty } from "../util";
import GameObject = UnityEngine.GameObject;
import Scene = UnityEngine.SceneManagement.Scene;

export function isViewHost(obj: ViewHost | GameObject): obj is ViewHost {
    return (<ViewHost>obj).isSceneHost != undefined;
}

export abstract class ViewHost {

    static create(target: GameObject | Scene): ViewHost {
        if ((<Scene>target).GetRootGameObjects != undefined) {
            return new ViewHostScene(<Scene>target);
        } else {
            return new ViewHostGO(<GameObject>target);
        }
    }

    abstract find(path: string): GameObject;
    abstract setActive(active: boolean): void;
    abstract destroy(cleanup: boolean): void;
    abstract get isSceneHost(): boolean;
}

export class ViewHostGO extends ViewHost {
    constructor(
        public gameObject: GameObject,
    ) { super(); }

    get isSceneHost(): boolean { return false; }

    find(path: string): GameObject {
        if (isNullOrEmpty(path)) {
            return this.gameObject;
        }
        
        const trans = this.gameObject.transform.Find(path);
        if (!trans) {
            throw new Error(`cannot find transform of path (${path}).`);
        }
        return trans.gameObject;
    }

    destroy(cleanup: boolean): void {
        if (this.gameObject && cleanup) {
            GameObject.Destroy(this.gameObject);
        }
    }

    setActive(active: boolean): void {
        this.gameObject.SetActive(active);
    }

}

export class ViewHostScene extends ViewHost {
    constructor(
        public scene: Scene,
    ) {
        super();
        const gos = scene.GetRootGameObjects();
        for (let i = 0; i < gos.Length; i++) {
            this.rootGameObjects.push(gos.get_Item(i));
        }
    }

    private rootGameObjects: GameObject[] = [];

    get isSceneHost(): boolean { return true; }

    find(path: string): GameObject {

        let first = path;
        let second = "";
        const idx = path.indexOf(`/`);
        if (idx >= 0) {
            first = path.substring(0, idx);
            second = path.substring(idx + 1);
        }

        const root = this.rootGameObjects.find(i => i.name == first);
        if (!root) {
            throw new Error(`cannot find transform of path (${path}) in scene.`);
        }

        if (isNullOrEmpty(second)) {
            return root;
        }

        const trans = root.transform.Find(second);
        if (!trans) {
            throw new Error(`cannot find transform of path (${path}).`);
        }
        return trans.gameObject;
    }

    destroy(cleanup: boolean): void {
        if (cleanup) {
            const res = container.resolve(ResourceService);
            res.unloadScene(this.scene);
        }
    }

    setActive(active: boolean): void {
        // nothing to do
        logger.debug(`scene set active ${active}`);
    }
}

const logger = getLogger(ViewHost.name);

