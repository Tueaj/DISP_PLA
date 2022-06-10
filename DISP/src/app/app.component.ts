import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { map, Subject } from 'rxjs';

export interface Order {
  items: Map<string, number>,
  creditId: string,
  price: number
}

export interface HttpCreateOrderRequest{
  creditId: string,
  orderedItems: Map<string, number>,
  creditRequired: number
}

export interface DialogData{
  item: string,
  amount: number
}


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent{
  displayedColumns: string[] = ['item', 'amount'];
  dataSource$ : Subject<Map<string, number>> = new Subject();
  tempData: Order = {
    items: new Map(),
    creditId: '',
    price: 0
  }; 

  dataSourceObservable = this.dataSource$.pipe(map(emit => Array.from(emit.entries())))

  constructor(public dialog: MatDialog, private httpClient: HttpClient) {
  }

  public addItemDialog(): void{
    const dialogRef = this.dialog.open(DialogOverviewExampleDialog, {
      width: '250px',
      data: {},
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
      this.tempData.items.set(result.item, result.amount);
      this.dataSource$.next(this.tempData.items);
    });
  }

  public async sendOrder(): Promise<void>{
    const httpRequest  = {
      creditId: this.tempData.creditId,
      orderedItems: Object.fromEntries(this.tempData.items),
      creditRequired: this.tempData.price
    }

    await this.httpClient.post('/api/order', httpRequest).toPromise();

    this.tempData.creditId = '';
    this.tempData.price = 0;
    this.tempData.items.clear();
    this.dataSource$.next(new Map());
  }

}

@Component({
  selector: 'dialog-overview-example-dialog',
  templateUrl: 'dialog-overview-example-dialog.html',
})
export class DialogOverviewExampleDialog {
  constructor(
    public dialogRef: MatDialogRef<DialogOverviewExampleDialog>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
  ) {}

  onNoClick(): void {
    this.dialogRef.close();
  }
}
