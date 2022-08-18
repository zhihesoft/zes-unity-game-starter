import { UnityEngine } from "csharp";
import { $typeof } from "puerts";
import { inject, injectable } from "tsyringe";
import { bind, component } from "../../core/view-decorators";
import { AfterViewInit } from "../../core/view-interfaces";
import { ViewRef, VIEW_DATA } from "../../core/view-ref";
import { ResourceService } from "../../services/resource-service";

@component({ template: "Assets/Bundles/ui-prefabs/controls/video-panel.prefab" })
@injectable()
export class VideoPanel implements AfterViewInit {

    constructor(
        private view: ViewRef,
        private res: ResourceService,
        @inject(VIEW_DATA) private video: string,
    ) { }

    @bind("") player!: UnityEngine.Video.VideoPlayer;

    @bind("close", { type: UnityEngine.UI.Button, event: "onClick", throttleSeconds: 1 })
    onClickSkip() {
        this.player.Stop();
        this.view.destroy();
    }

    async ngAfterViewInit() {
        const clip = await this.res.loadAsset<UnityEngine.Video.VideoClip>(this.video, $typeof(UnityEngine.Video.VideoClip));
        this.player.clip = clip;
        this.player.Play();
    }
}