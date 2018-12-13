import { AdvisorResponse } from "./advisorResponse";

export class HallOfFameResponse {
    year: number;
    month: number;
    advisors: AdvisorResponse[];

    constructor(){
        this.advisors = [];
    }
}