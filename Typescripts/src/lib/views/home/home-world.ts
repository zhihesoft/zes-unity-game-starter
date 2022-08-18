import { injectable } from "tsyringe";
import { component } from "../../core/view-decorators";

@component({ template: "Assets/Bundles/scenes/map_main.unity", node: "Scene" })
@injectable()
export class HomeWorld {

}