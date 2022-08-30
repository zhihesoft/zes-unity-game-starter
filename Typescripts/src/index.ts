import "reflect-metadata";
import { Zes } from "csharp";
import { container } from "tsyringe";
import { App } from "zes-unity-jslib";
import { AppComponent } from "./lib/app";

App.bootstrap(AppComponent, "App");
