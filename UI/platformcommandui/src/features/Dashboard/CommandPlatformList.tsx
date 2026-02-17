
import { useEffect, useState } from "react";
import type { CommandResponseSchema } from "../../types/commandService";
import { getCommandServiceCommands } from "../../api/commandService";
import { Button, Card, CardActions, CardContent, Typography } from "@mui/material";


export default function CommandPlatformList() {
    const [commands, setCommands] = useState<CommandResponseSchema[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getCommandServiceCommands()
            .then(data => {
                setCommands(data);
                setLoading(false);
            })
    }, []);

    if (loading) return <>Loading..</>;

    if (commands.length === 0) return <>Not data found</>;

    return (
        <>
            <Button variant="contained" sx={{ marginBottom: 1 }}>Create new command</Button>
            {commands.map(p =>
                <Card sx={{ minWidth: 275, marginBottom: 2 }} key={p.id}>
                    <CardContent>
                        <Typography variant="h5" component="div">
                            {p.commandLine}
                        </Typography>
                        <Typography gutterBottom sx={{ color: 'text.secondary', fontSize: 14 }}>
                            {p.howTo}
                        </Typography>
                        <Typography variant="body2">
                            {p.platformName}
                        </Typography>
                    </CardContent>
                    <CardActions>
                        <Button size="small">Edit</Button>
                    </CardActions>
                </Card>
            )}
        </>
    )
}
