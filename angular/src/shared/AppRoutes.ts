/**
 * Application Route Constants
 * Centralized route path definitions to avoid hardcoded strings throughout the application
 */

export class AppRoutes {
  // Root routes
  static readonly ROOT = "/";

  // Account routes
  static readonly ACCOUNT = {
    LOGIN: "/account/login",
    REGISTER: "/account/register",
    LOGIN_CALLBACK: "/login/callback",
  };

  // App routes
  static readonly APP = {
    HOME: "/app/home",
    UPDATE_PASSWORD: "/update-password",
  };

  // Admin routes
  static readonly ADMIN = {
    USERS: "/app/admin/users",
    ROLES: "/app/admin/roles",
    ROLES_CREATE: "/app/admin/roles/create",
    ROLES_EDIT: (id: number | string) => `/app/admin/roles/edit/${id}`,
    TENANTS: "/app/admin/tenants",
    CONFIGURATIONS: "/app/admin/configurations",
    MAIL: "/app/admin/mail",
    MEZON_WEBHOOKS: "/app/admin/mezon-webhooks",
  };

  // Category routes
  static readonly CATEGORIES = {
    EDUCATION_TYPES: "/app/categories/education-types",
    EDUCATIONS: "/app/categories/educations",
    SKILL: "/app/categories/skill",
    CV_SOURCES: "/app/categories/cv-sources",
    BRANCHES: "/app/categories/branches",
    JOB_POSITIONS: "/app/categories/job-positions",
    SUB_POSITIONS: "/app/categories/sub-positions",
    CAPABILITIES: "/app/categories/capabilities",
    CAPABILITY_SETTING: "/app/categories/capability-setting",
    CAPABILITY_SETTING_CAPABILITIES:
      "/app/categories/capability-setting/capabilities",
    POSITION_SETTING: "/app/categories/position-setting",
    POSTS: "/app/categories/posts",
    SCORE_SETTING: "/app/categories/score-setting",
    EXTERNAL_CV_DETAIL: (id: number | string) =>
      `/app/categories/external-cv/detail/${id}`,
  };

  // Candidate routes
  static readonly CANDIDATE = {
    STAFF_LIST: "/app/candidate/staff-list",
    STAFF_LIST_CREATE: "/app/candidate/staff-list/create",
    STAFF_LIST_DETAIL: (id: number | string) =>
      `/app/candidate/staff-list/${id}`,
    INTERN_LIST: "/app/candidate/intern-list",
    INTERN_LIST_CREATE: "/app/candidate/intern-list/create",
    INTERN_LIST_DETAIL: (id: number | string) =>
      `/app/candidate/intern-list/${id}`,
    VIEW_FILES: "/app/candidate/view-files",
    OFFER_LIST: "/app/candidate/offer-list",
    ONBOARD_LIST: "/app/candidate/onboard-list",
    INTERVIEW_LIST: "/app/candidate/interview-list",
    EXTERNAL_CV: "/app/candidate/external-cv",
    EXTERNAL_CV_DETAIL: (id: number | string) =>
      `/app/candidate/external-cv/detail/${id}`,
    APPLY_CV: "/app/candidate/apply-cv",
    APPLY_CV_CREATE: "/app/candidate/apply-cv/create",
  };

  // Requisition routes
  static readonly REQUISITION = {
    REQ_STAFF: "/app/requisition/req-staff",
    REQ_STAFF_DETAIL: (id: number | string) =>
      `/app/requisition/req-staff/${id}`,
    REQ_INTERN: "/app/requisition/req-intern",
    REQ_INTERN_DETAIL: (id: number | string) =>
      `/app/requisition/req-intern/${id}`,
    BASE: (path: string) => `/app/requisition/${path}`,
    DETAIL: (path: string, id: number | string) =>
      `/app/requisition/${path}/${id}`,
  };

  // NCC CV routes
  static readonly NCC_CV = {
    MY_PROFILE: "/app/ncc-cv/my-profile",
    WORKING_EXPERIENCE: "/app/ncc-cv/working-experience",
    PROJECT: "/app/ncc-cv/project",
    PROJECT_DETAIL: "/app/ncc-cv/project/detail-project",
    VERSION: "/app/ncc-cv/version",
    EMPLOYEE_LIST: "/app/ncc-cv/employee-list",
    EMPLOYEE_DETAIL: (id: number | string) =>
      `/app/ncc-cv/employee/detail-employee/${id}`,
    EMPLOYEE_POSITION: "/app/ncc-cv/employee-position",
    GROUP_SKILL: "/app/ncc-cv/group-skill",
    SKILL: "/app/ncc-cv/skill",
  };

  // Apply CV
  static readonly APPLY_CV = {
    ROOT: "/applycv",
  };

  // Relative routes (for use with relativeTo in routing)
  static readonly RELATIVE = {
    CREATE: "create",
    CAPABILITIES: "capabilities",
  };
}
