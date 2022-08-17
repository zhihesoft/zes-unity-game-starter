import { Liv } from "csharp";
import "reflect-metadata";
import { container } from "tsyringe";
import { ApplicationRef } from "./lib/core/application-ref";
import { LanguageService } from "./lib/services/language-service";
import { BUILD_CONFIG, GAME_CONFIG } from "./lib/views/global-values";
import { MainPage } from "./lib/views/main-page";


export function i18n(id: string): string {
    const lang = container.resolve(LanguageService);
    return lang.get(id);
}

// register global values
container.register(BUILD_CONFIG, { useValue: Liv.App.buildConfig });
container.register(GAME_CONFIG, { useValue: Liv.App.config });

container.resolve(ApplicationRef).bootstrap(MainPage);

