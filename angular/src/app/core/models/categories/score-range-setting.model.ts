import { ActionEnum } from "@shared/AppEnums";

export class ScoreSetting {
    id: number;
    userType: number;
    userTypeName: string;
    subPositionId: number;
    subPositionName: string;
    scoreRanges: ScoreRangeWithSetting[];
}

export class ScoreRangeWithSetting {
    id: number;
    scoreFrom: number;
    scoreTo: number;
    level: number;
    levelInfo: levelInfo;
}

export class levelInfo {
    defaultName: string;
    id: number;
    sortName: string;
    standardName: string;
}

export class ScoreSettingDialogData {
    dialogAction: ActionEnum;
    scoreSetting!: ScoreSetting;
    scoreRange!: ScoreRangeWithSetting;
}

export class FormFieldDisable {
    value: any;
    disabled?: boolean;
}

export class FormDialogCreateScoreSetting {
    userType?: number;
    subPositionId?: number;
    scoreTo: number;
    scoreFrom: number;
    level: number;
}

export class FormDialogUpdateScoreRange {
    scoreRangeId: number;
    scoreTo: number;
    scoreFrom: number;
    level: number;
}

export class ParamsGetScoreRange {
    userType: number;
    subPositionId: number;
}