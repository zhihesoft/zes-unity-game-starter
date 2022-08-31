import { singleton } from "tsyringe";

@singleton()
export class TestService {
    public test() {
        console.log("test function");
    }
}