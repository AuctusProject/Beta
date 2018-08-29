import { AssetSearch } from "./assetSearch";
import { AdvisorSearch } from "./advisorSearch";

export class SearchResponse {
    assets: AssetSearch[];
    advisors: AdvisorSearch[];

    constructor() {
        this.assets = [];
        this.advisors = [];
    }
}