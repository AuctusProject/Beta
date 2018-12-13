export class AdvisorPerformanceResponse {
    dailyDrawdown: number;
    bestTrade: number;
    worstTrade: number;
    dailyPerformance: DailyPerformanceResponse[];

    constructor() {
        this.dailyPerformance = [];
    }    
}
export class DailyPerformanceResponse {
    date: Date;
    equity: number;
    variation: number;
}