export class NotificationSetting {
    id: number;
    tenantId: number;
    type: number;
    typeName: string;
    webhookUrl: string;
    bodyMessage: string;
    description: string;
    isActived: boolean;
}

export class MessageTemplateDto {
    type: number;
    bodyMessage: string;
    propertiesSupport: string[];
}