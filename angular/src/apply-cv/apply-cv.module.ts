import { SharedModule } from './../shared/shared.module';
import { ApplyCvComponent } from "./apply-cv.component";
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientJsonpModule, HttpClientModule, HttpClient } from '@angular/common/http';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BrowserModule } from '@angular/platform-browser';
import { ToastModule } from 'primeng/toast';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    HttpClientJsonpModule,
    HttpClientModule,
    SharedModule,
    ModalModule.forRoot(),
    ToastModule,
    ReactiveFormsModule,
    BrowserAnimationsModule
  ],
  declarations: [
    ApplyCvComponent
  ]
})
export class ApplyCvModule { }
