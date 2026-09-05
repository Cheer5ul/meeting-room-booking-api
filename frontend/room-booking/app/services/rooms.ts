export interface RoomRequest {
    name: string;
    capacity: number;
    hasProjector: boolean;
    hasTv: boolean;
    hasWhiteboard: boolean;
}

export const getAllRooms = async() => {
    const response = await fetch("http://localhost:5211/api/rooms");

    return response.json();
}

// mehtods to interract with the API
export const createRoom = async(roomRequest: RoomRequest) => {
    await fetch("http://localhost:5211/api/rooms", {
        method: "POST",
        headers: {
            "content-type" : "application/json",
        },
        body: JSON.stringify(roomRequest)
    });
};

export const updateRoom = async (id: string, roomRequest: RoomRequest) => {
    await fetch(`http://localhost:5211/api/rooms/${id}`, {
        method: "PUT",
        headers: {
            "content-type" : "application/json",
        },
        body: JSON.stringify(roomRequest)
    });
};

export const deleteRoom = async (id: string, roomRequest: RoomRequest) => {
    await fetch(`http://localhost:5211/api/rooms/${id}`, {
        method: "DELETE"
    });
};