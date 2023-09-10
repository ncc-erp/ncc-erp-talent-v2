export class Mail {
    id: number;
    type: number;
    name: string;
    description: string;
    cCs: string;
    arrCCs: string[];
}
export class MailPreviewInfo {
    templateId: string;
    mailFuncType?: number;
    bodyMessage: string;
    subject: string;
    to: string;
    cCs: string[];
    propertiesSupport: string[];
}

export class MailDialogConfig {
    templateId?: number;
    mailInfo?: MailPreviewInfo;
    showEditBtn?: boolean;
    isAllowSendMail?: boolean;
}