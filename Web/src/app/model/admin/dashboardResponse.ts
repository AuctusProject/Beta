export class DashboardResponse {
    totalUsersStartedRegistration: number;
    totalUsersConfirmed: number;
    totalUsersStartedRegistrationFromReferral: number;
    totalUsersConfirmedFromReferral: number;
    totalAdvisors: number;
    totalRequestToBeAdvisor: number;
    totalActiveUsers: number;
    totalActiveAdvisors: number;
    totalWalletsWithAuc: number;
    totalWalletsInProgress: number;
    aucRatioPerConfirmedUser: number;
    aucRatioPerUserInProgress: number;
    aucHolded: number;
    aucHoldedInProgress: number;
    totalAssetsAdviced: number;
    totalRecentAdvices: number;
    totalUsersFollowing: number;
    totalAdvisorsFollowed: number;
    totalAdvices: number;
    totalFollowing: number;
    referralStatus: DistributionData[];
    advices: DistributionData[];
    following: DistributionData[];
    advisorReferral: AdvisorData[];
    advisorAdvices: AdvisorData[];
    advisorFollowers: AdvisorData[];
    usersStartedRegistration: RegistrationData[];
    usersConfirmed: RegistrationData[];
    advisors: RegistrationData[];
    requestToBeAdvisor: RegistrationData[];
    usersStartedRegistrationLastSitutation: FlagData;
    usersConfirmedLastSitutation: FlagData;
    advisorsLastSitutation: FlagData;
    requestToBeAdvisorLastSitutation: FlagData;

    constructor(){
    }
}
export class DistributionData {
    name: string;
    amount: number;
    constructor(){
    }
}
export class FlagData {
    description: string;
    date: Date;
    constructor(){
    }
}
export class RegistrationData {
    date: Date;
    value: Number;
    constructor(){
    }
}
export class AdvisorData {
    id: number;
    name: string;
    total: number;
    subValue1: number;
    subValue2: number;
    subValue3: number;
    constructor(){
    }
}