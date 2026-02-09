"use client";

import Button from "antd/es/button/";
import { useEffect } from "react";
import { getAllRooms} from "../services/rooms";
import { useState } from "react";
import { Rooms } from "../components/Rooms";
import Title from "antd/es/skeleton/";
import { CreateUpdateRoom } from "../components/CreateUpdateRoom";

export default function RoomsPage() {
    const[rooms, setRooms] = useState<Room[]>([]);
    const[loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        const getRooms = async() => {
            const rooms = await getAllRooms();
            setLoading(false);
            setRooms(rooms);
        };

        getRooms();
    }, []);

    return (
        <div>
            <Button>Add Room</Button>

            

            {loading ? <Title>Loading rooms...</Title>: (<Rooms rooms= {rooms}/>)}
        </div>
    );
}