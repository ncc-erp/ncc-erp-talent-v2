import { EMPLOYEE_PROFILE } from '@shared/AppConsts';
import { CatalogModel } from '../../core/models/common/common.dto';

export function findDegreeId(degreeType: string): number {
  return EMPLOYEE_PROFILE.Degree.find(item => item.name === degreeType).id;
}

export function findDegreeName(degreeType: number): string {
  return EMPLOYEE_PROFILE.Degree.find(item => item.id === degreeType).name;
}

export function enterName(name: string): any {
  const pattern = '^[_ ]*$';
  if (name.match(pattern)) {
    return true;
  } else {
    return false;
  }
}