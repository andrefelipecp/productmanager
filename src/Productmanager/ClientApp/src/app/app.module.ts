import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import './vendor';
import { ProductmanagerSharedModule } from 'app/shared/shared.module';
import { ProductmanagerCoreModule } from 'app/core/core.module';
import { ProductmanagerAppRoutingModule } from './app-routing.module';
import { ProductmanagerHomeModule } from './home/home.module';
import { ProductmanagerEntityModule } from './entities/entity.module';
// jhipster-needle-angular-add-module-import JHipster will add new module here
import { MainComponent } from './layouts/main/main.component';
import { NavbarComponent } from './layouts/navbar/navbar.component';
import { FooterComponent } from './layouts/footer/footer.component';
import { PageRibbonComponent } from './layouts/profiles/page-ribbon.component';
import { ErrorComponent } from './layouts/error/error.component';

@NgModule({
  imports: [
    BrowserModule,
    ProductmanagerSharedModule,
    ProductmanagerCoreModule,
    ProductmanagerHomeModule,
    // jhipster-needle-angular-add-module JHipster will add new module here
    ProductmanagerEntityModule,
    ProductmanagerAppRoutingModule,
  ],
  declarations: [MainComponent, NavbarComponent, ErrorComponent, PageRibbonComponent, FooterComponent],
  bootstrap: [MainComponent],
})
export class ProductmanagerAppModule {}
