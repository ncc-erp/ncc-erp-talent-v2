import { AppComponentBase } from 'shared/app-component-base';
import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { LazyLoadEvent } from 'primeng/api';

export class PagedResult {
    items: any[];
    totalCount: number;
}

export class PagedRequestDto {
    skipCount: number;
    maxResultCount: number;
    searchText: string;
    filterItems: Filter[] = [];
    sort: string;
    sortDirection: number;
}

export class Filter {
    propertyName: string;
    value: any;
    comparision: number;
    filterType?: number
    dropdownData?: any[]
}

export class ApiResponse<T>{
    result?: T;
    targetUrl?: string;
    success: boolean;
    error: HttpErrorResponse;
    unAuthorizedRequest?: boolean;
    loading: boolean;
}

@Component({
    template: ''
})
export abstract class PagedListingComponentBase<TEntityDto> extends AppComponentBase implements OnInit {
    public readonly GET_FIRST_PAGE = 1;
    public readonly defaultRows: number = 10;
    public readonly rowsPerPageOptions: number[] = [5, 10, 20, 50, 100];
    
    public first: number = 0;
    public pageSize: number = 5;
    public pageNumber: number = 1;
    public totalPages: number = 1;
    public totalItems: number;
    public searchText: string = '';
    public filterItems: Filter[] = [];
    public pageSizeType: number = 20;
    public advancedFiltersVisible: boolean = false;
    public sort: string = '';
    public sortDirection?: number;
  
    constructor(injector: Injector) {
        super(injector);
        this.route = injector.get(ActivatedRoute);
        this.router = injector.get(Router);
        this.route.queryParams.subscribe(params => {
            this.pageNumber = params['pageNumber'] ? params['pageNumber'] : 1;
            this.pageSize = params['pageSize'] ? params['pageSize'] : 20;
            this.searchText = params['searchText'] ? params['searchText'] : '';
            this.filterItems = params['filterItems'] ? JSON.parse(params['filterItems']) : [];
            this.advancedFiltersVisible = this.filterItems.length > 0;
            this.pageSizeType = Number(params['pageSize'] ? params['pageSize'] : 20);
        });
    }

    ngOnInit(): void {
        this.refresh();
    }

    checkAddFilter() {
        this.advancedFiltersVisible = !this.advancedFiltersVisible;
        if (this.filterItems.length === 0) {
            this.addFilter();
        }

    }

    lazyLoadingData(event: LazyLoadEvent) {
        this.setSortThead(event.sortField, event.sortOrder);
        this.pageNumber = event.first / this.pageSize + 1;
        (+this.pageSize !== event.rows) && (this.pageNumber = 1);
        this.pageSize = event.rows;
        this.refresh();
      }

    refresh(): void {
        this.getDataPage(this.pageNumber);
    }

    public showPaging(result: PagedResult, pageNumber: number): void {
        this.totalPages = ((result.totalCount - (result.totalCount % this.pageSize)) / this.pageSize) + 1;
        if (result.totalCount)
            this.totalItems = result.totalCount;
        else
            this.totalItems = 0;
        this.pageNumber = pageNumber;
        this.first = this.pageNumber === 1 ? 0 : this.first;
    }

    public getDataPage(page: number): void {
        const req = new PagedRequestDto();
        req.maxResultCount = this.pageSize;
        req.skipCount = (page - 1) * this.pageSize;
        req.filterItems = this.filterItems;
        
        if (this.sort) {
            req.sort = this.sort;
            req.sortDirection = this.sortDirection;
        }
        this.advancedFiltersVisible = this.filterItems.length > 0;
        req.searchText = this.searchText;
        this.pageNumber = page;
        this.router.navigate([], {
            queryParamsHandling: "merge",
            replaceUrl: true,
            queryParams: { pageNumber: this.pageNumber, pageSize: this.pageSize, searchText: this.searchText, filterItems: JSON.stringify(this.filterItems) }
        }).then(_ => this.list(req, page, () => {
        }));
    }

    public setSortThead(sort, sortDirection) {
        if (sort) {
            this.sort = sort;
            this.sortDirection = (sortDirection === -1) ? SortDirection.DESC : SortDirection.ASC;
        }
    }
 
    public deleteFilterItem(index: number) {
        this.filterItems.splice(index, 1);
    }
    public addFilter() {
        this.filterItems.push({
            propertyName: '',
            comparision: 0,
            value: '',
        });
    }
    public onEmitChange(event, i) {
        const { name, value } = event
        this.filterItems[i][name] = value
    }
    changePageSize() {
        if (this.pageSize > this.totalItems) {
            this.pageNumber = 1;
        }
        this.pageSize = this.pageSizeType;
        this.getDataPage(1);
    }
    AddFilterItem(request: PagedRequestDto, propertyName: string, value: any) {
        let filterList = request.filterItems
        if (value !== "" || value == 0) {
            filterList.unshift({ propertyName: propertyName, comparision: 0, value: value })
        }
        return filterList
    }
    clearFilter(request: PagedRequestDto, propertyName: string, value: any) {
        let filterList = request.filterItems
        if (value !== "" || value == 0) {
            let item = filterList.filter(item => item.propertyName === propertyName)[0]
            filterList.splice(request.filterItems.indexOf(item), 1)
        }
        return filterList
    }
    protected abstract list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void;
    protected abstract delete(entity: TEntityDto): void;

}

export enum SortDirection {
    ASC = 0,
    DESC = 1
}
