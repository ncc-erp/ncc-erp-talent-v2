import { MenuItem } from 'primeng/api';

export class BreadCrumbConfig {
    menuItem: MenuItem[];
    homeItem: MenuItem;
}

export class CatalogModel {
    id: number;
    name: string;
}

export class LevelInfo {
    id: number;
    defaultName: string;
    standardName: string;
    shortName: string;
}

export class UserCatalog extends CatalogModel {
    surName?: string;
    email?: string;
    fullName?: string;
    userName?: string;
}

export class InternSalaryCatalog {
    salary: number;
    id: number;
    defaultName: string;
    standardName: string;
    shortName: string;
}

export class PositionCatalog {
    id: number;
    position: string;
    items: { id: number, subPosition: string }[];
}