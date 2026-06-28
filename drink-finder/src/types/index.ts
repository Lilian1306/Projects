import {z} from 'zod'
import { CategoriesAPIResponseSchema,  SearchFitlerSchema, DrinksAPIResponse, type DrinkAPIResponse, type RecipeAPIResponseSchema } from '../utils/recipe-schema'

export type Categories = z.infer<typeof CategoriesAPIResponseSchema>
export type SearchFilter = z.infer<typeof SearchFitlerSchema>
export type Drinks = z.infer<typeof DrinksAPIResponse>
export type Drink = z.infer<typeof DrinkAPIResponse>
export type Recipe = z.infer<typeof RecipeAPIResponseSchema>