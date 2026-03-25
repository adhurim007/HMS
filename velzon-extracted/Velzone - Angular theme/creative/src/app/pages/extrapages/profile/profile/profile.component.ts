import { Component, OnInit, } from '@angular/core';
import { TokenStorageService } from '../../../../core/services/token-storage.service';
import { projectListModel, documentModel } from './profile.model';
import { document, projectList } from 'src/app/core/data';
import { PaginationService } from 'src/app/core/services/pagination.service';
import { UntypedFormBuilder } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})

/**
 * Profile Component
 */
export class ProfileComponent implements OnInit {

  projectList!: projectListModel[];
  document!: documentModel[];
  userData: any;
  allprojectList: any;

  constructor(private formBuilder: UntypedFormBuilder, private modalService: NgbModal, private TokenStorageService: TokenStorageService, public service: PaginationService) {
  }

  ngOnInit(): void {
    this.userData = this.TokenStorageService.getUser();
    /**
     * Fetches the data
     */
    this.fetchData();
  }

  /**
   * Fetches the data
   */
  private fetchData() {
    this.projectList = projectList;
    this.document = document;
    this.allprojectList = projectList;
  }

  /**
   * Swiper setting
   */
  config = {
    infinite: true,
    slidesToShow: 3,
    slidesToScroll: 1,
    autoplay: true,
    arrows: false
  };

  // Pagination
  changePage() {
    this.projectList = this.service.changePage(this.allprojectList)
  }

  /**
   * Confirmation mail model
   */
  deleteId: any;
  confirm(content: any, id: any) {
    this.deleteId = id;
    this.modalService.open(content, { centered: true });
  }

  // Delete Data
  deleteData(id: any) {
    this.document.slice(id, 1)
    this.modalService.dismissAll()
  }
}
