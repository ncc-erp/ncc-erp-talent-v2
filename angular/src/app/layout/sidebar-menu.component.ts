import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import {
    Router,
    RouterEvent,
    NavigationEnd,
    PRIMARY_OUTLET
} from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { MenuItem } from '@shared/layout/menu-item';
import { PERMISSIONS_CONSTANT } from '@shared/permission/permissions';

@Component({
    selector: 'sidebar-menu',
    templateUrl: './sidebar-menu.component.html'
})
export class SidebarMenuComponent extends AppComponentBase implements OnInit {
    menuItems: MenuItem[];
    menuItemsMap: { [key: number]: MenuItem } = {};
    activatedMenuItems: MenuItem[] = [];
    routerEvents: BehaviorSubject<RouterEvent> = new BehaviorSubject(undefined);
    homeRoute = '/app/home';

    constructor(injector: Injector) {
        super(injector);
        this.router.events.subscribe(this.routerEvents);
    }

    ngOnInit(): void {
        this.menuItems = this.getMenuItems();
        this.patchMenuItems(this.menuItems);
        this.routerEvents
            .pipe(filter((event) => event instanceof NavigationEnd))
            .subscribe((event) => {
                const currentUrl = event.url !== '/' ? event.url : this.homeRoute;
                const primaryUrlSegmentGroup = this.router.parseUrl(currentUrl).root
                    .children[PRIMARY_OUTLET];
                if (primaryUrlSegmentGroup) {
                    this.activateMenuItems('/' + primaryUrlSegmentGroup.toString());
                }
            });
    }

    getMenuItems(): MenuItem[] {
        return [
            new MenuItem(this.l('Home'), '/app/home', 'fas fa-home'),
            new MenuItem('Admin', '', 'fas fa-user-cog', PERMISSIONS_CONSTANT.TabAdmin, [
                new MenuItem(
                    this.l('Roles'),
                    '/app/admin/roles',
                    'fas fa-theater-masks',
                    PERMISSIONS_CONSTANT.Pages_Roles

                ),
                new MenuItem(
                    this.l('Tenants'),
                    '/app/admin/tenants',
                    'fas fa-building',
                    PERMISSIONS_CONSTANT.Pages_Tenants
                ),
                new MenuItem(
                    this.l('Users'),
                    '/app/admin/users',
                    'fas fa-users',
                    PERMISSIONS_CONSTANT.Pages_Users
                ),
                new MenuItem(
                    this.l('Configurations'),
                    '/app/admin/configurations',
                    'fas fa-cogs',
                    PERMISSIONS_CONSTANT.Pages_Configurations
                ),
                new MenuItem(
                    this.l('Mails'),
                    '/app/admin/mail',
                    'fas fa-envelope',
                    PERMISSIONS_CONSTANT.Pages_Mails
                )
            ]),
            new MenuItem('Categories', '', 'fas fa-folder-open', PERMISSIONS_CONSTANT.TabCategory, [
                new MenuItem(
                    'Education Types',
                    '/app/categories/education-types',
                    'fas fa-award',
                    PERMISSIONS_CONSTANT.Pages_EducationTypes
                ),
                new MenuItem(
                    'Educations',
                    '/app/categories/educations',
                    'fas fa-graduation-cap',
                    PERMISSIONS_CONSTANT.Pages_Educations
                ),
                new MenuItem(
                    'Skills',
                    '/app/categories/skill',
                    'fas fa-user-check',
                    PERMISSIONS_CONSTANT.Pages_Skills
                ),
                new MenuItem(
                    'CV Sources',
                    '/app/categories/cv-sources',
                    'fas fa-folder',
                    PERMISSIONS_CONSTANT.Pages_CVSources
                ),
                new MenuItem(
                    'Job Positions',
                    '/app/categories/job-positions',
                    'fas fa-id-card-alt',
                    PERMISSIONS_CONSTANT.Pages_JobPositions
                ),
                new MenuItem(
                    'Sub Positions',
                    '/app/categories/sub-positions',
                    'fa-user-tie fas nav-icon',
                    PERMISSIONS_CONSTANT.Pages_SubPositions
                ),
                new MenuItem(
                    'Branch',
                    '/app/categories/branches',
                    'far fa-building',
                    PERMISSIONS_CONSTANT.Pages_Branches
                ),
                new MenuItem(
                    'Capabilities',
                    '/app/categories/capabilities',
                    'fas fa-star',
                    PERMISSIONS_CONSTANT.Pages_Capabilities
                ),
                new MenuItem(
                    'Capability Setting',
                    '/app/categories/capability-setting',
                    'fas fa-tasks',
                    PERMISSIONS_CONSTANT.Pages_CapabilitySettings
                ),
                new MenuItem(
                    'Position Setting',
                    '/app/categories/position-setting',
                    'fas fa-cog',
                    PERMISSIONS_CONSTANT.Pages_PositionSettings
                ),
                new MenuItem(
                    this.l('Posts'),
                    '/app/categories/posts',
                    'fas fa-link',
                    PERMISSIONS_CONSTANT.Pages_Posts
                ),
                new MenuItem(
                    this.l('Score Setting'),
                    '/app/categories/score-setting',
                    'far fa-star',
                    PERMISSIONS_CONSTANT.Pages_ScoreSettings
                )
              
            ]),
            new MenuItem('Candidates', '', 'fas fa-id-card', 
                [PERMISSIONS_CONSTANT.Pages_CandidateStaff_ViewList, PERMISSIONS_CONSTANT.Pages_CandidateIntern_ViewList], [
                new MenuItem(
                    'Staff Candidate',
                    '/app/candidate/staff-list',
                    'fas fa-user-secret',
                    PERMISSIONS_CONSTANT.Pages_CandidateStaff_ViewList
                ),
                new MenuItem(
                    'Intern Candidate',
                    '/app/candidate/intern-list',
                    'fas fa-user-graduate',
                    PERMISSIONS_CONSTANT.Pages_CandidateIntern_ViewList
                ),
                new MenuItem(
                    this.l('External CV'),
                    '/app/candidate/external-cv',
                    'fas fa-id-badge',   
                    PERMISSIONS_CONSTANT.Pages_ExternalCVs
                ),
                new MenuItem(
                    this.l('Apply CV'),
                    '/app/candidate/apply-cv',
                    'fas fa-file-alt',
                  
                ),
            ]),
            new MenuItem('Requisitions', '', 'fas fa-file-audio', PERMISSIONS_CONSTANT.TabRequisition, [
                new MenuItem(
                    'Staff',
                    '/app/requisition/req-staff',
                    'fas fa-bullhorn',
                    PERMISSIONS_CONSTANT.Pages_RequisitionStaff
                ),
                new MenuItem(
                    'Intern',
                    '/app/requisition/req-intern',
                    'fas fa-satellite-dish',
                    PERMISSIONS_CONSTANT.Pages_RequisitionIntern
                ),
            ]),          
            new MenuItem('Candidate Interview', '/app/candidate/interview-list', 'fas fa-user-tag', PERMISSIONS_CONSTANT.TabInterview),
            new MenuItem('Candidate Offer', '/app/candidate/offer-list', 'fas fa-handshake', PERMISSIONS_CONSTANT.TabOffers),
            new MenuItem('Candidate Onboard', '/app/candidate/onboard-list', 'fas fa-briefcase', PERMISSIONS_CONSTANT.TabOnboard),
            new MenuItem('Report', '', 'fas fa-chart-line', PERMISSIONS_CONSTANT.TabReport, [
                new MenuItem(
                    'Recruitment Overview',
                    '/app/report/recruitment-overview',
                    'fas fa-table',
                    PERMISSIONS_CONSTANT.Pages_Reports_Overview
                ),
                new MenuItem(
                    'Staff Source',
                    '/app/report/staff-recruitment-performance',
                    'fas fa-chart-pie',
                    PERMISSIONS_CONSTANT.Pages_Reports_Staff_Performance
                ),
                new MenuItem(
                    'Intern Source',
                    '/app/report/intern-recruitment-performance',
                    'fas fa-chart-pie',
                    PERMISSIONS_CONSTANT.Pages_Reports_Intern_Performance
                ),
                new MenuItem(
                    'Intern Education',
                    '/app/report/education-intern',
                    'fas fa-chart-bar',
                    PERMISSIONS_CONSTANT.Pages_Reports_Intern_Education
                )
            ]),
            new MenuItem(this.l('Employee Profile'), '', 'fas fa-user-cog', PERMISSIONS_CONSTANT.TabFakeCV, [
                new MenuItem(
                    this.l('My Profile'),
                    '/app/ncc-cv/my-profile',
                    'fa-id-card far',
                    PERMISSIONS_CONSTANT.MyProfile
                ),
                new MenuItem(
                    this.l('Employee'),
                    '/app/ncc-cv/employee-list',
                    'fa-chalkboard-teacher fas',
                    PERMISSIONS_CONSTANT.Employee
                ),
                new MenuItem(
                    this.l('Work Exp'),
                    '/app/ncc-cv/working-experience',
                    'fa-building fas',
                    PERMISSIONS_CONSTANT.WorkingExperience
                ),
                new MenuItem(
                    this.l('Project'),
                    '/app/ncc-cv/project',
                    'fa-coins fas nav-icon',
                    PERMISSIONS_CONSTANT.Project
                ),
                new MenuItem(
                    this.l('Employee Position'),
                    '/app/ncc-cv/employee-position',
                    'fas fa-map',
                    PERMISSIONS_CONSTANT.EmployeePosition
                ),
                new MenuItem(
                    this.l('Group Skill'),
                    '/app/ncc-cv/group-skill',
                    'fas fa-user-check',
                    PERMISSIONS_CONSTANT.GroupSkill
                ),
                new MenuItem(
                    this.l('Skill'),
                    '/app/ncc-cv/skill',
                    'fas fa-code',
                    PERMISSIONS_CONSTANT.FakeSkill
                ),
            ]),
            new MenuItem('Guideline', 'https://docs.google.com/document/d/1wzMDnhIJCWGQ6cPlr4T5wEm4H64UcVYgw5abbOJfFDw', 'fas fa-book'),
        ];
    }

    patchMenuItems(items: MenuItem[], parentId?: number): void {
        items.forEach((item: MenuItem, index: number) => {
            item.id = parentId ? Number(parentId + '' + (index + 1)) : index + 1;
            if (parentId) {
                item.parentId = parentId;
            }
            if (parentId || item.children) {
                this.menuItemsMap[item.id] = item;
            }
            if (item.children) {
                this.patchMenuItems(item.children, item.id);
            }
        });
    }

    activateMenuItems(url: string): void {
        this.deactivateMenuItems(this.menuItems);
        this.activatedMenuItems = [];
        const foundedItems = this.findMenuItemsByUrl(url, this.menuItems);
        foundedItems.forEach((item) => {
            this.activateMenuItem(item);
        });
    }

    deactivateMenuItems(items: MenuItem[]): void {
        items.forEach((item: MenuItem) => {
            item.isActive = false;
            item.isCollapsed = true;
            if (item.children) {
                this.deactivateMenuItems(item.children);
            }
        });
    }

    findMenuItemsByUrl(
        url: string,
        items: MenuItem[],
        foundedItems: MenuItem[] = []
    ): MenuItem[] {
        items.forEach((item: MenuItem) => {
            if (item.route && url.includes(item.route)) {
                foundedItems.push(item);
            } else if (item.children) {
                this.findMenuItemsByUrl(url, item.children, foundedItems);
            }
        });
        return foundedItems;
    }

    activateMenuItem(item: MenuItem): void {
        item.isActive = true;
        if (item.children) {
            item.isCollapsed = false;
        }
        this.activatedMenuItems.push(item);
        if (item.parentId) {
            this.activateMenuItem(this.menuItemsMap[item.parentId]);
        }
    }

    isMenuItemVisible(item: MenuItem): boolean {
        if (!item.permissionName || !item.permissionName.length) {
            return true;
        }

        if(typeof item.permissionName === 'string') {
            return this.permission.isGranted(item.permissionName);
        }

        const permissions: string[] = item.permissionName;
        return !permissions.every(perName => !this.permission.isGranted(perName));
       
    }
}
