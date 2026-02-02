
import z from "zod";
export const platformResponse = {
    id: z.string(),
    name: z.string(),
    publisher: z.string(),
    cost: z.number().optional()
}

export type PlatformResponseSchema = z.infer<typeof platformResponse>;