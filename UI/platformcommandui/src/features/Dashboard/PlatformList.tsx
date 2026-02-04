import { useEffect, useState } from "react"
import type { PlatformResponseSchema } from "../../types/commandService";
import { getPlatformServicePlatforms } from "../../api/platformService";
import { Button, Card, CardActions, CardContent, Typography } from "@mui/material";



export default function PlatformList() {

  const [platforms, setPlatforms] = useState<PlatformResponseSchema[]>([]);

  useEffect(() => {
    getPlatformServicePlatforms()
      .then(data => {
        setPlatforms(data);
      })
  }, []);

  if (platforms.length === 0) return <>Loading..</>;

  return (
    <>
      {platforms.map(p =>
        <Card sx={{ minWidth: 275 }} key={p.id}>
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
            <Button size="small">Edit</Button>
          </CardActions>
        </Card>
      )}
    </>
  )
}
