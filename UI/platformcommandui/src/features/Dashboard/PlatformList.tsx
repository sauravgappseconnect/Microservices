import { useEffect, useState } from "react"
import { type PlatformResponseSchema } from "../../types/commandService";
import { getPlatformServicePlatforms } from "../../api/platformService";
import { Button, Card, CardActions, CardContent, Typography } from "@mui/material";
import Platform from "../Platforms/Platform";



export default function PlatformList() {

  const [platforms, setPlatforms] = useState<PlatformResponseSchema[]>([]);
  const [platformDetails, setShowPlatformDetails] = useState<PlatformResponseSchema | undefined>(undefined);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getPlatformServicePlatforms()
      .then(data => {
        setPlatforms(data);
        setLoading(false);
      })
  }, []);

  function showPlatformDetails(value: PlatformResponseSchema | undefined) {
    setShowPlatformDetails(value);
  }

  if (loading) return <>Loading..</>;

  if (platforms.length === 0) return <>Not data found</>;

  return (
    <>
      <Button variant="contained" sx={{ marginBottom: 1 }}
        onClick={() => {
          showPlatformDetails({ id: "", name: "", publisher: "", cost: 0 } as PlatformResponseSchema);
        }}>Create new platform</Button>
      {platforms.map(p =>
        <Card sx={{ minWidth: 275, marginBottom: 2 }} key={p.id}>
          <CardContent>
            <Typography variant="h5" component="div">
              {p.publisher}
            </Typography>
            <Typography gutterBottom sx={{ color: 'text.secondary', fontSize: 14 }}>
              {p.name}
            </Typography>
            <Typography variant="body2">
              Cost: {p.cost}
            </Typography>
          </CardContent>
          <CardActions>
            <Button size="small" onClick={() => { showPlatformDetails(p) }}>Edit</Button>
          </CardActions>
        </Card>
      )}
      {platformDetails ? <Platform platformDetails={platformDetails} setShowPlatformDetails={setShowPlatformDetails} /> : <></>}
    </>
  )
}
