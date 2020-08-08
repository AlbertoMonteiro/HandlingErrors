import * as moment from "moment";

export class DateTimeValueConverter {
    toView(value: string, format?: string) {
        var momentDate = moment(value);

        if (format === "fromNow")
            return momentDate.fromNow();

        return momentDate.format("DD/MM/YYYY");
    }
}