import Card from "antd/es/card/Card";
import {CardName} from "./CardName";
import Button from "antd/es/button";

interface props {
    rooms: Room[];
}

export const Rooms = ( {rooms} : props) => {
    return(
        <div className="cards">
            {rooms.map((room: Room) => (
                <Card 
                    key={room.id}
                    title={<CardName name={room.name} capacity={room.capacity} />}
                    bordered={false}
                >
                    <p>{room.hasProjector}</p>
                    <div className="card__buttons">
                        <Button>Edit</Button>
                        <Button>Delete</Button>
                    </div>
                </Card>
            ))}
        </div>
    );
}