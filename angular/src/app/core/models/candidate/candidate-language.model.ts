import { ActionEnum } from '../../../../shared/AppEnums';
import { Category } from '../categories/categories.model';

export  class CandidateLanguage extends Category {
  colorCode: string;
  alias: string;
  note: string;
  }
  
  export class CandidateLanguageConfigDiaLog {
    candidateLanguage: CandidateLanguage;
    action: ActionEnum;
  }
