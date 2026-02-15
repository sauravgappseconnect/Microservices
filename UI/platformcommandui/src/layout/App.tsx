
import { Box, Container, Grid, Typography } from "@mui/material";
import PlatformList from "../features/Dashboard/PlatformList";
import CommandPlatformList from "../features/Dashboard/CommandPlatformList";
import Navbar from "./Navbar";

function App() {

    return (
        <>
            <Navbar />
            <Container maxWidth={false} disableGutters sx={{ mt: 3, px: 2, height: "calc(100vh - 80px)" }}>
                <Grid container spacing={2} sx={{ height: "100%" }}>
                    <Grid size={6} sx={{ height: "100%" }}>
                        <Typography variant="h4">Platforms</Typography>
                        <Box sx={{
                            height: "calc(100% - 40px)",
                            overflowY: "auto",
                            pr: 1
                        }}>
                            <PlatformList />
                        </Box>

                    </Grid>
                    <Grid size={6} sx={{ height: "100%" }}>
                        <Typography variant="h4">Commands</Typography>
                        <Box sx={{
                            height: "calc(100% - 40px)",
                            overflowY: "auto",
                            pr: 1
                        }}>
                            <CommandPlatformList />
                        </Box>

                    </Grid>
                </Grid>
            </Container>
        </>

    )
}

export default App
