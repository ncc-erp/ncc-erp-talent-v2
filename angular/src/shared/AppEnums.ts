import { TenantAvailabilityState } from '@shared/service-proxies/service-proxies';


export class AppTenantAvailabilityState {
    static Available: number = TenantAvailabilityState._1;
    static InActive: number = TenantAvailabilityState._2;
    static NotFound: number = TenantAvailabilityState._3;

}

export enum CreationTimeEnum {
    ALL = 'All',
    DAY = 'Day',
    WEEK = "Week",
    MONTH = "Month",
    Quarter = "Quarter",
    HALF_YEAR = "Half-year",
    YEAR = "Year",
    CUSTOM = "Custom"
}

export enum UserType {
    INTERN = 0,
    STAFF = 1
}

export enum SearchType {
    AND = 'and',
    OR = 'or'
}

export enum ActionEnum {
    CREATE = "Create",
    UPDATE = "Update",
    CLONE = "Clone",
    CLOSE_AND_CLONE = "Close & Clone",
    PREVIEW = "Preview",
    SELECT = "Select",
}

export enum StatusEnum {
    InProgress = "InProgress",
    Closed = "Closed",
}

export enum CategoryEnum {
    EDUCATION_TYPE = "Education Type",
    EDUCATION = "Education",
    SKILL = "Skill",
    CV_SOURCE = 'CV Source',
    JOB_POSITION = 'Job Position',
    CAPABILITY = 'Capability',
}

export enum ToastMessageType {
    WARN = 'warn',
    ERROR = 'error',
    SUCCESS = 'success',
    INFO = 'info'
}

export enum API_RESPONSE_STATUS {
    SUCCESS = 'Success',
    FAILD = 'Faild'
}

export enum COMPARISION_OPERATOR {
    Equal = 0,
    LessThan = 1,
    LessThanOrEqual = 2,
    GreaterThan = 3,
    GreaterThanOrEqual = 4,
    NotEqual = 5,
    Contains = 6, //For string
    StartsWith = 7, //For string
    EndsWith = 8, //For string
    In = 9 // For ListItem
}

export enum REQUEST_CV_STATUS {
    AddedCV = 0,
    ScheduledTest = 1,
    FailedTest = 2,
    PassedTest = 3,
    ScheduledInterview = 4,
    RejectInterview = 5,
    PassedInterview = 6,
    FailedInterview = 7,
    AcceptedOffer = 8,
    RejectedOffer = 9,
    Onboarded = 10,
    RejectedTest = 11,
    RejectedApply = 12,
}

export enum PriorityColor {
    Critical = '#D9001B',
    High = '#F59A23',
    Medium = '#70B603',
    Low = '#81D3F8',
    Default = '#12b886'
}

export enum RequisitionStatusColor {
    InProgress = '#70b603',
    Closed = '#636464'
}

export enum BranchColor {
    HN = '#D9001B',
    Vinh = '#F59A23',
    DN = '#70B603',
    HCM = '#81D3F8',
    Default = '#12b886'
}

export enum CandidateStatus {
    Contacting = '#F59E0B',
    Failed = '#EF4444',
    Passed = '#25bb1a',
}

export enum RequestCvStatusColor {
    AddedCV = "#7ebd1c",
    ScheduledTest = "#50C76B",
    FailedTest = "#555555",
    PassedTest = "#28a745",
    ScheduledInterview = "#2499BF",
    RejectInterview = "#808000",
    PassedInterview = "#027db4",
    FailedInterview = "#333333",
    AcceptedOffer = "#f59a23",
    RejectedOffer = "#D9001B",
    Onboarded = "#e74c3c",
    Default = '#12b886',
    RejectedTest = "#8D13E1",
    RejectedApply = "#00FFFF",
}

export enum UserTypeColor {
    Staff = "#02a7f0",
    Internship = "#70b603",
    Default = '#12b886'
}

export enum LevelColor {
    Intern_0 = '#D7D7D7',
    Intern_1 = '#AAAAAA',
    Intern_2 = '#555555',
    Intern_3 = '#333333',
    FresherMinus = '#81D3F8',
    Fresher = '#02A7F0',
    FresherPlus = '#027DB4',
    JuniorMinus = '#95F204',
    Junior = '#70B603',
    JuniorPlus = '#4B7902',
    MiddleMinus = '#F59A23',
    Middle = '#B8741A',
    MiddlePlus = '#7B4D12',
    SeniorMinus = '#EC808D',
    Senior = '#D9001B',
    Principal = '#A30014',
    Default = '#8400FF'
}

export enum SortType {
    DESC = -1,
    ASC = 1
}

export enum DefaultRoute {
    Admin = "/app/admin/roles",
    Category = '/app/categories/education-types',
    ExternalCV = '/app/candidate/external-cv',
    Candidate = '/app/candidate/staff-list',
    Requisition = '/app/requisition/req-staff',
    Report = '/app/report/recruitment-overview',
    Employee_Profile = '/app/ncc-cv/my-profile',
    ProjectManagement = ''
}

export enum CANDIDATE_DETAILT_TAB_DEFAULT {
    PERSONAL_INFO = 0,
    EDUCATION = 1,
    SKILLS = 2,
    CURRENT_REQ = 3,
    APPLY_HISTORY = 4
}

export enum CV_REFERENCE_TYPE {
    STAFF = 0,
    EDUCATION = 1
}

export enum DIFF_DATE_COLOR {
    FUTURE = '#02830f',
    PAST = '#D32F2F'
}

export enum MAIL_TYPE {
    AcceptJob = 0,
    FailCV = 1,
    FailInterview = 2,
    InviteInternship = 3,
    InviteInterview = 4,
    InviteTest = 5,
    ResultTest = 6
}
