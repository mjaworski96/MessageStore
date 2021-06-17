import {Id} from './common';

export interface AliasWithId {
  id: number;
  name: string;
  internal: boolean;
  application: string;
  inApplicationId: string;
  members: AliasMember[];
}

export interface AliasMember {
  id: number;
  name: string;
  application: string;
  inApplicationId: number;
}

export interface AliasWithIdList {
  aliases: AliasWithId[];
}

export interface EditAlias {
  name: string;
  members: Id[];
}

export interface EditAliasName {
  name: string;
}
