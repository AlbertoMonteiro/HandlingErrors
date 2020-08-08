import { autoinject } from "aurelia-framework";
import { Router } from "aurelia-router";
import { ValidationController, ValidationRules } from "aurelia-validation";
import { HttpClient } from "aurelia-fetch-client";
import { EventAggregator } from "aurelia-event-aggregator";
import { RouteNames, ToastMessage } from "../app";

@autoinject
export class ListaRecados {
    heading: string = "Adicionar um recado";
    recado = new RecadoInputModel();

    constructor(private readonly http: HttpClient,
        private readonly eventAggregator: EventAggregator,
        private readonly validationController: ValidationController,
        private readonly router: Router) { }

    public async enviarRecado() {
        var resultadoValidacao = await this.validationController.validate();
        if (!resultadoValidacao.valid)
            return;

        var resultado = await this.http.fetch("recados", {
            method: "POST",
            body: JSON.stringify(this.recado),
            headers: {
                "Content-Type": "application/json"
            }
        });
        if (resultado.status === 201) {
            for (var prop in this.recado) {
                if (this.recado.hasOwnProperty(prop))
                    this.recado[prop] = null;
            }
            this.exibirToast("Um novo recado foi criado!");
            this.router.navigateToRoute(RouteNames.listaRecados);
        } else {
            this.exibirToast("Aconteceu um erro e seu recado não pode ser salvo.");
        }
    }

    private exibirToast(newLocal: string) {
        const toastMessage: ToastMessage = { message: newLocal };
        this.eventAggregator.publish("showToast", toastMessage);
    }

    public canDeactivate(): boolean | undefined {
        const nenhumDadoInserido = [this.recado.assunto, this.recado.destinatario, this.recado.mensagem, this.recado.remetente]
            .every(x => x == null || x.trim().length === 0);
        if (nenhumDadoInserido)
            return true;
        return confirm("Você já preencheu algo, deseja mesmo cancelar?");
    }
}

export class RecadoInputModel {
    remetente: string;
    destinatario: string;
    assunto: string;
    mensagem: string;
}

ValidationRules
    .ensure((r: RecadoInputModel) => r.remetente).displayName("De").required().maxLength(50)
    .ensure((r: RecadoInputModel) => r.destinatario).displayName("Para").required().maxLength(50)
    .ensure((r: RecadoInputModel) => r.assunto).required().maxLength(100)
    .ensure((r: RecadoInputModel) => r.mensagem).required().maxLength(500)
    .on(RecadoInputModel);