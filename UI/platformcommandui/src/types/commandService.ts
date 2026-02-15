
import z from "zod";

export const platformResponse = z.object({
    id: z.string(),
    name: z.string(),
    publisher: z.string(),
    cost: z.number().optional()
});
export type PlatformResponseSchema = z.infer<typeof platformResponse>;

export const commandResponse = z.object({
    id: z.string(),
    createdAt: z.string(),
    updatedAt: z.string(),
    createdBy: z.string().optional(),
    howTo: z.string(),
    commandLine: z.string(),
    platformName: z.string()
});
export type CommandResponseSchema = z.infer<typeof commandResponse>;


