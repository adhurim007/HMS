import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-privacy-ploicy',
  templateUrl: './privacy-ploicy.component.html',
  styleUrls: ['./privacy-ploicy.component.scss']
})
export class PrivacyPloicyComponent implements OnInit {

  // bread crumb items
  breadCrumbItems!: Array<{}>;

  constructor() { }

  ngOnInit(): void {
    /**
     * BreadCrumb
     */
    this.breadCrumbItems = [
      { label: 'Pages' },
      { label: 'Privacy Policy', active: true }
    ];
  }

}
