import "reflect-metadata";
import { App } from "zes-unity-jslib";
import { AppComponent } from "./lib/app_component";

App.bootstrap(AppComponent, "App");

export const i18n = App.i18n;