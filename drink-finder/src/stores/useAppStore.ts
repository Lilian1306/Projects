import { create } from "zustand";
import { createRecipeSlice, type RecipeSliceType } from "./recipeSlice";
import { devtools } from "zustand/middleware";
import { createFavoritesSlice, type FavoritesSliceType } from "./favoriteSlice";
import { createNotificationSlice, type NotificatioSliceType } from "./notificationSlice";


export const useAppStore = create<RecipeSliceType & FavoritesSliceType & NotificatioSliceType>()(devtools((...a) => ({
   ...createRecipeSlice(...a),
   ...createFavoritesSlice(...a),
   ...createNotificationSlice(...a)
})))