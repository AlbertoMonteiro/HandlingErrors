import { autoinject } from "aurelia-framework";
import { PLATFORM } from "aurelia-pal";
import { EventAggregator } from "aurelia-event-aggregator";
import { Router, RouterConfiguration } from "aurelia-router";
import { HttpClient } from "aurelia-fetch-client";
import * as $ from "jquery";
import { ValidationController, validationMessages } from "aurelia-validation";
import { BootstrapFormRenderer } from "./resources/bootstrapFormRenderer";

@autoinject
export class App {
    public router: Router;
    toastMessage: string;

    constructor(private readonly http: HttpClient,
        private readonly validationController: ValidationController,
        private readonly eventAggregator: EventAggregator) {
        http.configure(config => {
            config
                .useStandardConfiguration()
                .withBaseUrl("api/");
        });

        validationController.addRenderer(new BootstrapFormRenderer());
    }

    attached() {
        $(".toast").toast({ delay: 2500 });
        this.eventAggregator.subscribe("showToast", (data: ToastMessage) => {
            this.toastMessage = data.message;
            $(".toast").toast("show");
        });
    }

    public configureRouter(config: RouterConfiguration, router: Router) {
        config.title = "Recados";
        config.map([
            {
                route: ["", "recados"],
                name: RouteNames.listaRecados,
                moduleId: PLATFORM.moduleName("./recados/listaRecados"),
                nav: true,
                title: "Lista Recados"
            },
            {
                route: "recados/adicionar",
                name: RouteNames.adicionarRecado,
                moduleId: PLATFORM.moduleName("./recados/adicionarRecado"),
                nav: true,
                title: "Adicionar recado"
            }
        ]);

        this.router = router;
    }
}

export const RouteNames = {
    listaRecados: "listaRecados",
    adicionarRecado: "adicionarRecado"
}

export interface ToastMessage {
    message: string;
}

validationMessages["required"] = "${$displayName} é obrigatório";
validationMessages["maxLength"] = "${$displayName} deve ter no máximo ${$config.length} caracteres";