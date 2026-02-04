
import { Container, Grid, Typography } from "@mui/material";
import PlatformList from "../features/Dashboard/PlatformList";
import CommandPlatformList from "../features/Dashboard/CommandPlatformList";
import Navbar from "./Navbar";

function App() {

    return (
        <>
            <Navbar />
            <Container maxWidth={false} disableGutters sx={{ mt: 3, px: 2 }}>
                <Grid container spacing={2}>
                    <Grid size={6}>
                        <Typography variant="h4">Platforms</Typography>
                        <PlatformList />
                    </Grid>
                    <Grid size={6}>
                        <Typography variant="h4">Commands</Typography>
                        <CommandPlatformList />
                    </Grid>
                </Grid>
            </Container>
        </>

    )
}

export default App
