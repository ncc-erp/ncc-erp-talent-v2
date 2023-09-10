import {
  HttpClient
} from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ApiResponse } from "../../../../shared/paged-listing-component-base";
import { UserCatalog } from '../../models/common/common.dto';

import { BaseApiService } from "../apis/base-api.service";

@Injectable({
  providedIn: "root",
})
export class CandidateInterviewService extends BaseApiService {
  constructor(public http: HttpClient) {
    super(http);
  }

  changeUrl(): string {
    return "Interview";
  }

  getAllInterview(): Observable<ApiResponse<UserCatalog[]>> {
    return this.getAll("/GetAllInterview");
  }

}
