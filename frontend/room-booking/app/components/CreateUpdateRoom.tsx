
import { Input, Modal, Checkbox } from "antd";
import { RoomRequest } from "../services/rooms";
import { useEffect, useState } from "react";

//props for create and update room
interface Props {
    mode: Mode;
    values: Room;
    isModelOpen: boolean;
    handleCancel: () => void;
    handleCreate: (request: RoomRequest) => void;
    handleUpdate: (id: string, request: RoomRequest) => void;
}

export enum Mode {
    Create,
    Edit,
}

export const CreateUpdateRoom = ({
    mode, 
    values, 
    isModelOpen, 
    handleCancel, 
    handleCreate,
    handleUpdate
}: Props) => {
    const[name, setName] = useState<string>("");
    const[capacity, setCapacity] = useState<number>(1);
    const[hasProjector, setHasProjector] = useState<boolean>(false);
    const[hasTv, setHasTv] = useState<boolean>(false);
    const[hasWhiteboard, setHasWhiteboard] = useState<boolean>(false);

    useEffect(() => {
        setName(values.name);
        setCapacity(values.capacity);
        setHasProjector(values.hasProjector);
        setHasTv(values.hasTv);
        setHasWhiteboard(values.hasWhiteboard);
    }, [values]);

    const handleOnOk = async() => {
        const roomRequest = {name, capacity, hasProjector, hasTv, hasWhiteboard};

        mode == Mode.Create ? handleCreate(roomRequest) : handleUpdate(values.id, roomRequest);
    }

    return (
        <Modal title={
            mode === Mode.Create ? "Add Room" : "Edit Room"} 
            open={isModelOpen} 
            cancelText={"Cancel"}
            onOk={handleOnOk}
            onCancel={handleCancel}
        >
                <div className="room_modal">
                    <Input
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        placeholder="Name"
                    />
                    {/* <TextArea
                        value={descripton}
                        onChange={(e) => setDescription(e.target.value)}
                        autoSize={{ minRows: 3, maxRows: 3 }}
                        placeholder="description"
                    /> */}
                    <Input
                        value={capacity}
                        onChange={(e) => setCapacity(Number(e.target.value))}
                        placeholder="Capacity"
                    />
                    <Checkbox
                        checked={hasProjector}
                        onChange={(e) => setHasProjector(e.target.checked)}
                    >
                        Has Projector
                    </Checkbox>
                    <Checkbox
                        checked={hasTv}
                        onChange={(e) => setHasTv(e.target.checked)}
                    >
                        Has TV
                    </Checkbox>
                    <Checkbox
                        checked={hasWhiteboard}
                        onChange={(e) => setHasWhiteboard(e.target.checked)}
                    >
                        Has Whiteboard
                    </Checkbox>
                </div>
        </Modal>
    );
};