import { UnityEngine } from "csharp";
import { singleton } from "tsyringe";

@singleton()
export class FadeService {

    private readonly fadeDuration = 0.2;

    private image!: UnityEngine.UI.Image;

    init(img: UnityEngine.UI.Image) {
        this.image = img;
    }

    in(): Promise<void> {
        this.image.gameObject.SetActive(true);
        return new Promise(resolve => {
            this.image.DOFade(0, this.fadeDuration).onComplete = () => {
                this.image.gameObject.SetActive(false);
                resolve();
            };
        });
    }

    out(): Promise<void> {
        this.image.gameObject.SetActive(true);
        return new Promise(resolve => {
            this.image.DOFade(1, this.fadeDuration).onComplete = () => {
                resolve();
            };
        });
    }
}