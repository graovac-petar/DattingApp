import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_model/member';
import { of, tap } from 'rxjs';
import { Photo } from '../_model/photo';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http=inject(HttpClient);
  baseUrl = environment.apiUrl;
  members = signal<Member[]>([]);

  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'AppUsers').subscribe({
      next: (members) => {
        this.members.set(members);
      }
    })
  }

  getMember(username:string){
    const member = this.members().find(x => x.userName === username);
    if( member!==undefined) return of(member);

    return this.http.get<Member>(this.baseUrl + 'AppUsers/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'AppUsers', member).pipe(
      tap(()=>{
        this.members.update(members => members.map(x => x.userName === member.userName ? member : x))
      })
    )
  }

  setMainPhoto(photo: Photo) {
    return this.http.put(this.baseUrl + 'AppUsers/set-main-photo/' + photo.id, {}).pipe(
      tap(()=>{
        this.members.update(members => members.map(m => {
          if(m.photos.includes(photo)){
            m.photoUrl=photo.url
          }
          return m;
        }))
      })
    )
  }

  deletePhoto(photo: Photo){
    return this.http.delete(this.baseUrl + 'AppUsers/delete-photo/' + photo.id).pipe(
      tap(()=>{
        this.members.update(members => members.map(m => {
          if(m.photos.includes(photo)){
            m.photos=m.photos.filter(x=>x.id!==photo.id)
          }
          return m;
        }))
      })
    )
  }
}
