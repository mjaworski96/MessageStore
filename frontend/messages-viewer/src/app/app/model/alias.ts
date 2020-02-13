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
