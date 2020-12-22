import {Id} from './common';

export interface AliasWithId {
  id: number;
  name: string;
  internal: boolean;
  application: string;
  inApplicationId: string;
}
export interface AliasWithIdList {
  aliases: AliasWithId[];
}

export interface EditAlias {
  name: string;
  members: Id[];
}
