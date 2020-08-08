import { autoinject, computedFrom } from "aurelia-framework";
import { HttpClient } from "aurelia-fetch-client";
import { RecadoViewModel } from "resources/ApiModels";

@autoinject
export class Users {
    heading: string = "Recados";
    recados: RecadoViewModel[] = [];
    recadosFilhos: RecadoViewModel[];

    @computedFrom("recadosFilhos")
    get filhosCarregados() {
        return this.recadosFilhos?.length > 0;
    }

    constructor(private readonly http: HttpClient) {
    }

    async activate(): Promise<void> {
        const response = await this.http.fetch("recados/?$filter=AgrupadoComId eq null");
        this.recados = (await response.json()).map(r => ({ selected: false, ...r }));
    }

    async selecionar(recado: RecadoViewModel) {
        this.recados.forEach(r => r.selected = false);
        recado.selected = true;
        const response = await this.http.fetch(`recados/?$filter=AgrupadoComId eq ${recado.id} or Id eq ${recado.id}`);
        this.recadosFilhos = await response.json();
    }
}
