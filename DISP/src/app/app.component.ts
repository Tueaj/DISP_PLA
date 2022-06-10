import {Component, Inject, OnInit} from '@angular/core';
import {MatDialog, MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import { BehaviorSubject, map, Observable, Subject } from 'rxjs';

export interface PeriodicElement {
  item: string;
  amount: number;
}

export interface Order {
  items: PeriodicElement[],
  orderId: string,
  price: number
}


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent{
  displayedColumns: string[] = ['item', 'amount'];
  dataSource$ : Subject<PeriodicElement[]> = new Subject();
  tempData: Order; 

  constructor(public dialog: MatDialog) {
    this.tempData = {} as Order;
  }

  public addItemDialog(): void{
    const dialogRef = this.dialog.open(DialogOverviewExampleDialog, {
      width: '250px',
      data: {},
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
      this.tempData.items.push(result);
      this.dataSource$.next(this.tempData.items);
    });
  }

  public sendOrder(): void{
    console.log(this.tempData);
  }

}

@Component({
  selector: 'dialog-overview-example-dialog',
  templateUrl: 'dialog-overview-example-dialog.html',
})
export class DialogOverviewExampleDialog {
  constructor(
    public dialogRef: MatDialogRef<DialogOverviewExampleDialog>,
    @Inject(MAT_DIALOG_DATA) public data: PeriodicElement,
  ) {}

  onNoClick(): void {
    this.dialogRef.close();
  }
}
