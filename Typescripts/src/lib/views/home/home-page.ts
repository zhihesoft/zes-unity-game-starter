import { Liv, UnityEngine } from "csharp";
import { injectable } from "tsyringe";
import { bind, component } from "../../core/view-decorators";
import { AfterViewInit, OnInit } from "../../core/view-interfaces";
import { ViewRef } from "../../core/view-ref";
import { Transition } from "../../core/view-transition";
import { LoadingPanel } from "../common/loading-panel";
import { HomeScene } from "./home-scene";

@component({ template: "Assets/Bundles/ui-prefabs/home/home-page.prefab", transition: Transition.Fade })
@injectable()
export class HomePage implements AfterViewInit, OnInit {
    constructor(
        private view: ViewRef,
    ) { }

    @bind("") inputHandler!: Liv.InputHandler;

    private scene?: HomeScene;
    private loading?: ViewRef;

    async ngOnInit() {
        this.loading = await this.view.showChild(LoadingPanel);
        this.inputHandler.moving = v => this.onMoving(v);
    }

    async ngAfterViewInit() {
        this.scene = await this.view.showChild(HomeScene).then(v => v.component);
        this.scene?.player?.setSpeed(5);
        // await waitForSeconds(10);
        this.loading?.destroy();
    }

    private onMoving(v2: UnityEngine.Vector2) {
        this.scene?.player?.moving.next(v2);
    }
}

// const logger = getLogger(HomePage.name);
