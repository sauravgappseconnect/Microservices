
import { useEffect, useState } from "react";
import { getCommandServicePlatforms } from "../api/commandService";
import { getPlatformServicePlatforms } from "../api/platformService";
import { type PlatformResponseSchema } from "../types/commandService";
import { Container } from "@mui/material";
import PlatformList from "../features/Dashboard/PlatformList";
import CommandPlatformList from "../features/Dashboard/CommandPlatformList";

function App() {
    const [commandPlatforms, setCommandPlatforms] = useState<PlatformResponseSchema[]>([]);

    useEffect(() => {
        getCommandServicePlatforms()
            .then(data => {
                setCommandPlatforms(data);
            });
    }, []);

    if (!commandPlatforms) return <>Loading...</>

    return (
        <Container fixed>
            <PlatformList />
            <CommandPlatformList />
        </Container>
    )
}

export default App
