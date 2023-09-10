import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ProfileEducation, TechnicalExpertise, WorkingExperience } from '@app/core/models/employee-profile/profile-model';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { Observable } from 'rxjs';
import { BaseApiService } from '../apis/base-api.service';

@Injectable({
  providedIn: 'root'
})
export class MyProfileService extends BaseApiService {

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return 'MyProfile';
  }

  /**
   * 
   * @param userId 
   * @returns 
   * {
      "name": "string",
      "surname": "string",
      "phoneNumber": "string",
      "emailAddressInCV": "string",
      "imgPath": "string",
      "currentPosition": "string",
      "userId": 0,
      "address": "string",
      "branch": "string"
    }
   */
  getUserGeneralInfo(userId: number, versionId: number): Observable<ApiResponse<any>> {
    return this.get(`/GetUserGeneralInfo?userId=${userId}&versionId=${versionId}`);
  }

  /**
   * 
   * @param userId 
   * @returns 
   * [
      {
        "cvcandidateId": 0,
        "cvemployeeId": 0,
        "schoolOrCenterName": "string",
        "degreeType": 0,
        "major": "string",
        "startYear": "stri",
        "endYear": "stri",
        "description": "string",
        "order": 0,
        "id": 0
      }
    ]
   */
  getEducationInfo(userId: number, versionId: number): Observable<ApiResponse<any>> {
    return this.get(`/GetEducationInfo?userId=${userId}&versionId=${versionId}`);
  }


  /**
   * 
   * @param userId 
   * @returns 
   * {
      "personalAttributes": [
        "string"
      ]
    }
   */
  getPersonalAttribute(userId: number): Observable<ApiResponse<any>> {
    return this.get(`/getPersonalAttribute?userId=${userId}`);
  }

  
  getUserWorkingExperience(userId: number, versionId: number): Observable<ApiResponse<any>> {
    return this.get(`/GetUserWorkingExperience?userId=${userId}&versionId=${versionId}`);
  }

  getTechnicalExpertise(userId: number, versionId: number): Observable<ApiResponse<any>> {
    return this.get(`/GetTechnicalExpertise?userId=${userId}&versionId=${versionId}`);
  }

  updateOrderWorkingExperience(payload: { id: number, order: number }[]): Observable<ApiResponse<any>> {
    return this.update(payload, '/UpdateOrderWorkingExperience');
  }

  updateOrderEducation(payload: { id: number, order: number }[]): Observable<ApiResponse<any>> {
    return this.update(payload, '/UpdateOrderEducation');
  }

  updateWorkingExperience(data: Object): Observable<ApiResponse<any>> {
    return this.update(data, '/UpdateWorkingExperience');
  }

  updatePersonalAttribute(payload: { personalAttributes: string[] }): Observable<ApiResponse<any>> {
    return this.update(payload, '/UpdatePersonalAttribute');
  }

  updateTechnicalExpertise(payload: TechnicalExpertise): Observable<ApiResponse<any>> {
    return this.update(payload, '/UpdateTechnicalExpertise');
  }

  saveUserGeneralInfo(payload: FormData): Observable<ApiResponse<any>> {
    return this.create(payload, '/SaveUserGeneralInfo');
  }

  saveEducation(payload: ProfileEducation): Observable<ApiResponse<ProfileEducation>> {
    return this.create(payload, '/SaveEducation');
  }

  deleteWorkingExperience(id: number): Observable<ApiResponse<string>> {
    return this.delete(id, '/DeleteWorkingExperience?id=' + id);
  }

  deleteEducation(id: number): Observable<ApiResponse<string>> {
    return this.delete(id, '/DeleteEducation?id=' + id);
  }
}
