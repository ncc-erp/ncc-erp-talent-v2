import { FormGroup } from '@angular/forms';
import { CV_EXTENSION_ALLOW, IMAGE_EXTENSION_ALLOW } from '@shared/AppConsts';
import * as _ from "lodash";

export function checkNumber(value: number) {
  return value !== undefined && value !== null && !Number.isNaN(value);
}

export function getFormControlValue(form: FormGroup, formControlName: string) {
  return form.controls[formControlName]?.value ?? null;
}

export function checkArrayUndefined(value: any) {
  return value == undefined ? [] : value
}

export function copyObject(source: any) {
  return source ? _.cloneDeep(source) : null;
}

export function isImageExtensionAllow(file: File) {
  return IMAGE_EXTENSION_ALLOW.includes(file.type);
}

export function isCVExtensionAllow(file: File) {
  if(!file){ return; }
  const extensionFile = file.name.split('.').pop();
  return CV_EXTENSION_ALLOW.includes(extensionFile);
}

export function randomHexColor(): string {
  let n = (Math.random() * 0xfffff * 1000000).toString(16);
  return '#' + n.slice(0, 6);
}

export function nullToEmpty(value): string {
  return value ?? '';
}

export function convertPhoneNumber(number: string): string {
  let resultNumber: string;
  if (number) {
    resultNumber = number.replace(/[^\d]/g, "");
    if (resultNumber.slice(0, 2) === "84") {
      resultNumber = resultNumber.substring(2);
    }
    if (resultNumber.slice(0, 1) !== "0") {
      resultNumber = `0${resultNumber}`;
    }
  }
  return resultNumber;
}