export class AppConsts {

    static autoBotServiceBaseUrl: string;
    static remoteServiceBaseUrl: string;
    static appBaseUrl: string;
    static appBaseHref: string; // returns angular's base-href parameter value if used during the publish
    static googleClientAppId: string;
    static enableNormalLogin: boolean;
    static backendIsNotABP: boolean;

    static localeMappings: any = [];

    static readonly userManagement = {
        defaultAdminUserName: 'admin'
    };

    static readonly localization = {
        defaultLocalizationSourceName: 'TalentV2'
    };

    static readonly authorization = {
        encryptedAuthTokenName: 'enc_auth_token'
    };

    static readonly userBranchStyle = {
        0: "badge badge-pill badge-danger",
        1: "badge badge-pill badge-success",
        2: "badge badge-pill badge-primary",
        3: "badge badge-pill badge-warning",
    }
    static readonly userTypeStyle = {
        0: "badge badge-success",
        1: "badge badge-primary",
        2: "badge badge-danger",
        3: "badge badge-warning",
        4: "badge badge-secondary",
        5: "badge vendor-userType"
    }
    static readonly userLevleStyle =
        {
            Intern_0: 0,
            Intern_600K: 1,
            Intern_2M: 2,
            Intern_4M: 3,
            FresherMinus: 4,
            Fresher: 5,
            FresherPlus: 6,
            JuniorMinus: 7,
            Junior: 8,
            JuniorPlus: 9,
            MiddleMinus: 10,
            Middle: 11,
            MiddlePlus: 12,
            SeniorMinus: 13,
            Senior: 14,
            SeniorPlus: 15,
        }
}

export const RADIO_YES_OR_NO = [
    { value: true, name: 'Yes' },
    { value: false, name: 'No' }
]

export const RADIO_DONE_OR_NOTDONE = [
    { value: true, name: 'Done' },
    { value: false, name: 'Not Done' }
]

export const CREATION_TIME = ["Week", "Month", "Quarter", "Year", "Custom"];
export const INTERVIEW_TIME = ["Day", "Week", "Month", "Year", "Custom"];
export const FILTER_TIME = ["Day", "Week", "Month", "Quarter", "Half-year", "Year", "Custom"];

export const MESSAGE = {
    ADD_SUCCESS: "Add was successfully",
    ADD_FAILD: "Add was failed",
    CREATE_SUCCESS: "Create was successfully",
    CREATE_FAILD: "Create was failed",
    UPDATE_SUCCESS: "Update was successfully",
    UPDATE_FAILED: "Update was failed",
    DELETE_FAILED: "Delete was failed",
    DELETE_SUCCES: "Delete was successfully",
    CLONE_SUCCESS: "Clone was successfully",
    CLOSE_SUCCESS: "Close was successfully",
    REOPEN_SUCCESS: "Reopen was successfully",
    APPLY_CV_SUCCESS: "You have submitted the application form successfully!",
    EXTRACT_CV_WARN: "CV information extraction function is not configured. Please contact the administrator",
    EXTRACTING_CV: "Extracting CV information...."
}

export const DateFormat = {
    YYYY_MM_DD: 'YYYY/MM/DD',
    YYYY_MM_DD_H_MM_SS: 'YYYY/MM/DD h:mm:ss',
    YYYY_MM_DD_HH_MM_SS: 'YYYY/MM/DD H:mm:ss',
    DD_MM_YYYY: 'DD/MM/YYYY',
    DD_MM_YYYY_H_MM: 'DD/MM/YYYY H:mm',
    H_MM_SS: 'h:mm:ss',
    MM_YYYY: 'MM/YYYY',
    DD_MM: 'DD/MM',
    YYYY: 'YYYY',
    DD: 'DD',
    MM: 'MM',
}

export const DateOption = {
    DAYS_PER_WEEK: 7
}

export const EMPLOYEE_PROFILE = {
    Degree: [
        { id: 0, name: 'HighSchool' },
        { id: 1, name: 'Bachelor' },
        { id: 2, name: 'Master' },
        { id: 3, name: 'PostDoctor' },
        { id: 4, name: 'Certificate' }
    ],
    DanhSach: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
}
export const PROCESS_STATUS_OPTION = {
    UNPROCESS_CV : 1,
    OVERDUE_CV : 2,
    UNPROCESS_SEND_MAIL : 3,
    OVERDUE_SEND_MAIL: 4,
};
export const PROCESS_STATUS = [
    { id: 1, name: "Unprocessed CV"},
    { id: 2, name: "Overdue CV"},
  ];
export const RQ_PROCESS_STATUS = [
    { id: 3, name: "Unprocessed Send Mail"},
    { id: 4, name: "Overdue Send Mail"},
];
export const IMAGE_EXTENSION_ALLOW = ['image/jpg', 'image/jpeg', 'image/gif', 'image/png'];

export const CV_EXTENSION_ALLOW = ['doc', 'docx', 'xlsx', 'csv', 'pdf'];

export const TOOL_URL = {
    contest: 'https://contest.ncc.asia',
    lms: 'http://lms.nccsoft.vn'
}
