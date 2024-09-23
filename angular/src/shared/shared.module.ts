import { CommonModule } from "@angular/common";
import { NgModule, ModuleWithProviders } from "@angular/core";
import { RouterModule } from "@angular/router";
import { NgxPaginationModule } from "ngx-pagination";

import { AppSessionService } from "./session/app-session.service";
import { AppUrlService } from "./nav/app-url.service";
import { AppAuthService } from "./auth/app-auth.service";
import { AppRouteGuard } from "./auth/auth-route-guard";
import { LocalizePipe } from "@shared/pipes/localize.pipe";

import { AbpPaginationControlsComponent } from "./components/pagination/abp-pagination-controls.component";
import { AbpValidationSummaryComponent } from "./components/validation/abp-validation.summary.component";
import { AbpModalHeaderComponent } from "./components/modal/abp-modal-header.component";
import { AbpModalFooterComponent } from "./components/modal/abp-modal-footer.component";
import { LayoutStoreService } from "./layout/layout-store.service";

import { BusyDirective } from "./directives/busy.directive";
import { EqualValidator } from "./directives/equal-validator.directive";
import { CustomTooltipDirective } from "./directives/custom-tooltip.directive";

import { MatCardModule } from '@angular/material/card'
import { MatCheckboxModule } from '@angular/material/checkbox'
import { MatChipsModule } from '@angular/material/chips'
import { MatDatepickerModule } from '@angular/material/datepicker'
import { MatDialogModule } from '@angular/material/dialog'
import { MatDividerModule } from '@angular/material/divider'
import { MatExpansionModule } from '@angular/material/expansion'
import { MatGridListModule } from '@angular/material/grid-list'
import { MatIconModule } from '@angular/material/icon'
import { MatInputModule } from '@angular/material/input'
import { MatListModule } from '@angular/material/list'
import { MatMenuModule } from '@angular/material/menu'
import { MatNativeDateModule } from '@angular/material/core'
import { MatPaginatorModule } from '@angular/material/paginator'
import { MatRadioModule } from '@angular/material/radio'
import { MatTableModule } from '@angular/material/table'
import { MatTabsModule } from '@angular/material/tabs'
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxStarsModule } from 'ngx-stars';
import { ListFilterPipe } from './pipes/list-filter.pipe';
import { FilterComponent } from './filter/filter.component';
import { UserInfoComponent } from './components/user-info/user-info.component';
import { TableModule } from 'primeng/table';
import { TreeModule } from 'primeng/tree';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { DialogService, DynamicDialogModule } from 'primeng/dynamicdialog';
import { ButtonModule } from 'primeng/button';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { InputTextModule } from 'primeng/inputtext';
import { AutoFocusDirective } from './directives/auto-focus.directive';
import { DialogActionToolbarComponent } from './components/dialog-action-toolbar/dialog-action-toolbar.component';
import { DropdownModule } from 'primeng/dropdown';
import { PickListModule } from 'primeng/picklist';
import { DialogModule } from 'primeng/dialog';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { MultiSelectModule } from 'primeng/multiselect';
import { DateSelectorComponent } from './components/date-selector/date-selector.component';
import { CustomDateDialogComponent } from './components/date-selector/custom-date-dialog/custom-date-dialog.component';
import { CalendarModule } from 'primeng/calendar';
import { SelectButtonModule } from 'primeng/selectbutton';
import { TagModule } from 'primeng/tag';
import { MultiSelectHRadioComponent } from './components/multi-select-h-radio/multi-select-h-radio.component';
import { DateTimePipe } from './pipes/date-time.pipe';
import { TabViewModule } from 'primeng/tabview';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { RadioButtonModule } from 'primeng/radiobutton';
import { InputMaskModule } from 'primeng/inputmask';
import { MenuModule } from 'primeng/menu';
import { CheckboxModule } from 'primeng/checkbox';
import { PanelModule } from 'primeng/panel';
import { ChipModule } from 'primeng/chip';
import { ChipsModule } from 'primeng/chips';
import { RatingModule } from 'primeng/rating';
import { TagSelectComponent } from './components/tag-select/tag-select.component';
import { DisplayRatingComponent } from './components/display-rating/display-rating.component';
import { InputNumberModule } from 'primeng/inputnumber';
import { ColorPickerModule } from 'primeng/colorpicker';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { EditorModule } from 'primeng/editor';
import { TreeSelectModule } from 'primeng/treeselect';
import { SliderModule } from 'primeng/slider';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { PasswordModule } from 'primeng/password';

import { CreateCandidateComponent } from './pages/create-candidate/create-candidate.component';
import { PersonalInfoComponent } from './pages/create-candidate/personal-info/personal-info.component';
import { CandidateEducationComponent } from './pages/create-candidate/candidate-education/candidate-education.component';
import { CandidateSkillComponent } from './pages/create-candidate/candidate-skill/candidate-skill.component';
import { CurrentRequisitionComponent } from './pages/create-candidate/current-requisition/current-requisition.component';
import { ResizeContentDirective } from './directives/resize-content.directive';
import { RequisitionDetailComponent } from './pages/requisition-detail/requisition-detail.component';
import { LoopNumberPipe } from './pipes/loop-number.pipe';
import { UploadAvatarComponent } from './components/upload-avatar/upload-avatar.component';
import { ImageCropperModule } from 'ngx-image-cropper';
import { ApplicationHistoryComponent } from './pages/create-candidate/application-history/application-history.component';
import { LoopArrayPipe } from './pipes/loop-array.pipe';
import { RequisitionInfoComponent } from './components/requisition-info/requisition-info.component';
import { CandidateInfoNewComponent } from './components/candidate-info-new/candidate-info-new.component';
import { SectionBreadcrumbComponent } from './components/section-breadcrumb/section-breadcrumb.component';
import { PositionDropdownComponent } from './components/position-dropdown/position-dropdown.component';
import { PageErrorComponent } from './pages/page-error/page-error.component';
import { SafeHtmlPipe } from '@shared/pipes/safe-html.pipe';

import { ExportCandidateComponent } from './components/export-candidate/export-candidate.component'
import { TabsModule } from "ngx-bootstrap/tabs";
import { ExportDialogComponent } from './components/export-dialog/export-dialog.component'
import { ConfirmPresentForHr } from "./pages/create-candidate/current-requisition/confirm-presentforhr/confirm-presentforhr.component";
import { NgxDocViewerModule } from "ngx-doc-viewer";
import { ViewFllesComponent } from "./pages/create-candidate/view-flles/view-flles.component"
import { PdfJsViewerModule } from "ng2-pdfjs-viewer";
import { PdfDocViewerComponent } from './components/pdf-doc-viewer/pdf-doc-viewer.component';
import { InputSwitchModule } from 'primeng/inputswitch';
import { RadioSwitchComponent } from './components/talent-radio-switch/talent-radio-switch.component';
//import { CurrentRequesitionGuildlineDialogComponent } from './pages/create-candidate/current-requisition/current-requesition-guildline-dialog/current-requesition-guildline-dialog.component';
const primengLibs = [
  TableModule,
  TreeModule,
  BreadcrumbModule,
  DynamicDialogModule,
  DialogModule,
  ButtonModule,
  ConfirmDialogModule,
  InputTextModule,
  DropdownModule,
  PickListModule,
  InputTextareaModule,
  MultiSelectModule,
  CalendarModule,
  SelectButtonModule,
  TagModule,
  TabViewModule,
  ProgressSpinnerModule,
  RadioButtonModule,
  InputMaskModule,
  MenuModule,
  CheckboxModule,
  PanelModule,
  ChipModule,
  RatingModule,
  InputNumberModule,
  ColorPickerModule,
  OverlayPanelModule,
  EditorModule,
  TreeSelectModule,
  ChipsModule,
  SliderModule,
  AutoCompleteModule,
  PasswordModule,
  InputSwitchModule
]

const materialLibs = [
  MatCardModule,
  MatCheckboxModule,
  MatChipsModule,
  MatDatepickerModule,
  MatDialogModule,
  MatDividerModule,
  MatExpansionModule,
  MatGridListModule,
  MatIconModule,
  MatInputModule,
  MatListModule,
  MatMenuModule,
  MatNativeDateModule,
  MatPaginatorModule,
  MatRadioModule,
  MatTableModule,
  MatTabsModule,
  MatFormFieldModule,
  MatSelectModule,
];

const candidate = [
  CandidateInfoNewComponent,
  CreateCandidateComponent,
  PersonalInfoComponent,
  CandidateEducationComponent,
  CandidateSkillComponent,
  CurrentRequisitionComponent,
  ApplicationHistoryComponent,
  ExportCandidateComponent,
  ConfirmPresentForHr,
  ViewFllesComponent,
]

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    NgxPaginationModule,
    FormsModule,
    NgxStarsModule,
    DragDropModule,
    ToastModule,
    FormsModule,
    TabsModule,
    ReactiveFormsModule,
    ImageCropperModule,
    NgxDocViewerModule,
    PdfJsViewerModule,
    ...materialLibs,
    ...primengLibs
  ],
    declarations: [
        AbpPaginationControlsComponent,
        AbpValidationSummaryComponent,
        AbpModalHeaderComponent,
        AbpModalFooterComponent,
        LocalizePipe,
        BusyDirective,
        EqualValidator,
        ListFilterPipe,
        FilterComponent,
        UserInfoComponent,
        AutoFocusDirective,
        DialogActionToolbarComponent,
        DateSelectorComponent,
        CustomDateDialogComponent,
        MultiSelectHRadioComponent,
        DateTimePipe,
        TagSelectComponent,
        DisplayRatingComponent,
        ...candidate,
        ResizeContentDirective,
        RequisitionDetailComponent,
        LoopNumberPipe,
        UploadAvatarComponent,
        LoopArrayPipe,
        RequisitionInfoComponent,
        SectionBreadcrumbComponent,
        PositionDropdownComponent,
        PageErrorComponent,
        SafeHtmlPipe,
        ExportDialogComponent,
        CustomTooltipDirective,
        PdfDocViewerComponent,
        RadioSwitchComponent
        //CurrentRequesitionGuildlineDialogComponent,
    ],
    exports: [
        AbpPaginationControlsComponent,
        AbpValidationSummaryComponent,
        AbpModalHeaderComponent,
        AbpModalFooterComponent,
        LocalizePipe,
        DateTimePipe,
        BusyDirective,
        EqualValidator,
        NgxStarsModule,
        DragDropModule,
        ToastModule,
        FormsModule,
        ReactiveFormsModule,
        AutoFocusDirective,
        DialogActionToolbarComponent,
        DateSelectorComponent,
        MultiSelectHRadioComponent,
        TagSelectComponent,
        DisplayRatingComponent,
        ResizeContentDirective,
        LoopNumberPipe,
        UploadAvatarComponent,
        RequisitionInfoComponent,
        ImageCropperModule,
        SectionBreadcrumbComponent,
        LoopArrayPipe,
        PositionDropdownComponent,
        ExportDialogComponent,
        ...materialLibs,
        ...primengLibs,
        ...candidate,
        SafeHtmlPipe,
        CustomTooltipDirective,
        RadioSwitchComponent
    ]
})
export class SharedModule {
  static forRoot(): ModuleWithProviders<SharedModule> {
    return {
      ngModule: SharedModule,
      providers: [
        AppSessionService,
        AppUrlService,
        AppAuthService,
        AppRouteGuard,
        LayoutStoreService,
        MessageService,
        DialogService,
        ConfirmationService,
      ],
    };
  }
}
